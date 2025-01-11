package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import com.mythicpalette.andropen.R
import com.mythicpalette.andropen.data.EventType
import com.mythicpalette.andropen.data.PointerInfo
import com.mythicpalette.andropen.data.PointerType
import com.mythicpalette.andropen.data.serialize
import com.mythicpalette.andropen.helpers.Settings
import com.mythicpalette.andropen.helpers.SocketHandler
import java.nio.ByteBuffer
import java.nio.ByteOrder

/**
 * TODO: document your custom view class.
 */
enum class BorderCoverStyle(val value: Int) {
    Shortest(0), Longest(1), Independent(2);

    companion object {
        fun fromValue(value: Int): BorderCoverStyle {
            return values().find { it.value == value }
                ?: throw IllegalArgumentException("Invalid value for BorderStyle: $value")
        }
    }
}

class TouchInputView : View {
    private var penHovering: Boolean = false
    private var penTouching: Boolean = false

    var onTouch: (MutableList<PointerInfo>) -> Unit = {}
    var onHover: (PointerInfo) -> Unit = {}

    var borderCoverStyle: BorderCoverStyle = BorderCoverStyle.Shortest
    var borderCoverage: Float = 1f

    constructor(context: Context, attrs: AttributeSet) : super(context, attrs) {
        init(attrs);
    }
    constructor(context: Context, attrs: AttributeSet, defStyle: Int) : super(
        context,
        attrs,
        defStyle
    ) {
        init(attrs);
    }

    private fun init(attrs: AttributeSet) {
        // Read custom attributes
        context.theme.obtainStyledAttributes(
            attrs,
            R.styleable.TouchInputView,
            0, 0
        ).apply {
            try {
                val strokeWidth = getDimension(R.styleable.TouchInputView_strokeWidth, 4f)
                val borderColor = getColor(R.styleable.TouchInputView_borderColor, Color.WHITE)
                borderCoverage = getFloat(R.styleable.TouchInputView_borderCoverage, 1f)
                borderCoverStyle = BorderCoverStyle.fromValue(getInt(R.styleable.TouchInputView_borderCoverageStyle, 0))

                borderPaint.apply {
                    color = borderColor
                    this.strokeWidth = strokeWidth
                    style = Paint.Style.STROKE
                }

            } finally {
                recycle()
            }
        }
    }

    private val borderPaint = Paint().apply {
        color = Color.WHITE  // Border color
        strokeWidth = 4f    // Border thickness
        style = Paint.Style.STROKE
    }

    private val path = android.graphics.Path()

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)

        val cornerX: Float = this.width.toFloat()
        val cornerY: Float = this.height.toFloat()

        if ( this.borderCoverage == 0f )
            return

        var vLen = this.height*(this.borderCoverage * 0.5f)
        var hLen = this.width*(this.borderCoverage * 0.5f)

        if ( this.borderCoverStyle == BorderCoverStyle.Shortest )
        {
            // If we're using the shortest side then find the shorter value and duplicate
            if ( vLen < hLen )
                hLen = vLen
            else
                vLen = hLen
        }
        else if ( this.borderCoverStyle == BorderCoverStyle.Longest )
        {
            // If we're using the longest side then find the longer value and duplicate
            if ( vLen > hLen )
                hLen = vLen
            else
                vLen = hLen
        }

        // Top left corner
        canvas.drawLine(0f, 0f, hLen, 0f, borderPaint)
        canvas.drawLine(0f, 0f, 0f, vLen, borderPaint)

        // Top right corner
        canvas.drawLine(cornerX - hLen, 0f, cornerX, 0f, borderPaint)
        canvas.drawLine(cornerX, 0f, cornerX, vLen, borderPaint)

        // Bottom left corner
        canvas.drawLine(0f, cornerY - vLen, 0f, cornerY, borderPaint);
        canvas.drawLine(0f, cornerY, hLen, cornerY, borderPaint);

        // Bottom right corner
        canvas.drawLine(cornerX, cornerY - vLen, cornerX, cornerY, borderPaint);
        canvas.drawLine(cornerX - hLen, cornerY, cornerX, cornerY, borderPaint);
    }

    override fun onTouchEvent(ev: MotionEvent): Boolean {
        val infos: MutableList<PointerInfo> = mutableListOf()
        val action = ev.actionMasked
        val stamp = System.currentTimeMillis()
        val viewWidth = this.width
        val viewHeight = this.height

        // Iterate over all pointers to handle pen input (pressure, movement, etc.)
        for (i in 0 until ev.pointerCount) {
            val pi = PointerInfo(
                pointerId = ev.getPointerId(i),          // Pointer ID
                x = ev.getX(i), y = ev.getY(i), // Coordinates
                timeStamp = stamp,              // Timestamp
                pressure = ev.getPressure(i),
                tiltX = ev.getAxisValue(MotionEvent.AXIS_TILT, i),
                tiltY = ev.getAxisValue(MotionEvent.AXIS_TILT, i),
                viewWidth = viewWidth,          // View resolution
                viewHeight = viewHeight
            )

            // Check if the input device is a pen or a finger
            when (ev.getToolType(i)) {
                MotionEvent.TOOL_TYPE_STYLUS -> {
                    when (action) {
                        MotionEvent.ACTION_DOWN -> {
                            pi.eventType = EventType.DOWN     // Event Type
                            penTouching = true
                        }
                        MotionEvent.ACTION_UP -> {
                            pi.eventType = EventType.UP
                            penTouching = false
                        }
                        MotionEvent.ACTION_POINTER_UP -> {
                            pi.eventType = EventType.POINTER_UP
                            penTouching = false
                        }
                    }
                }

                MotionEvent.TOOL_TYPE_FINGER -> {
                    // If the pen is in use, disable touching.
                    if ( Settings.PenBlocksTouch && ( penHovering || penTouching )) continue

                    pi.pointerType = PointerType.TOUCH;

                    // Finger touch detected (tap or gesture)
                    when (action) {
                        MotionEvent.ACTION_DOWN -> pi.eventType = EventType.DOWN
                        MotionEvent.ACTION_POINTER_DOWN -> pi.eventType = EventType.POINTER_DOWN
                        MotionEvent.ACTION_UP -> pi.eventType = EventType.UP
                        MotionEvent.ACTION_POINTER_UP -> pi.eventType = EventType.POINTER_UP
                    }
                }
                else -> {}
            }
            infos.add(pi)
        }
        this.onTouch(infos)
        return true
        //
        //
        //
//        when (event.action) {
//            MotionEvent.ACTION_DOWN -> {
//                path.moveTo(event.x, event.y)
//                return true
//            }
//            MotionEvent.ACTION_MOVE -> {
//                path.lineTo(event.x, event.y)
//                invalidate() // Redraw the view
//            }
//        }
//        return super.onTouchEvent(event)
    }

    override fun onHoverEvent(ev: MotionEvent): Boolean {
        val pi = PointerInfo(
            pointerId = ev.getPointerId(0),
            eventType = EventType.HOVER_ENTER,
            x = ev.getX(0),
            y = ev.getY(0),
            pressure = 0f,
            pointerType = PointerType.PEN,
            tiltX = ev.getAxisValue(MotionEvent.AXIS_TILT, 0),
            tiltY = ev.getAxisValue(MotionEvent.AXIS_TILT, 0),
            timeStamp = System.currentTimeMillis(),
            viewWidth = this.width,
            viewHeight = this.height
        );

        when (ev.action) {
            MotionEvent.ACTION_HOVER_ENTER -> penHovering = true;
            MotionEvent.ACTION_HOVER_MOVE -> pi.eventType = EventType.HOVER_MOVE
            MotionEvent.ACTION_HOVER_EXIT -> {
                penHovering = false
                pi.eventType = EventType.HOVER_EXIT
            }
        }

        this.onHover(pi);
        return true
    }

    fun clearCanvas() {
        path.reset()
        invalidate() // Redraw the view
    }
}