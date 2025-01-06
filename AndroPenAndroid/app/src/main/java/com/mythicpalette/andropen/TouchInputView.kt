package com.mythicpalette.andropen

import android.content.Context
import android.content.Context.SENSOR_SERVICE
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.hardware.Sensor
import android.hardware.SensorManager
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import android.widget.Button
import androidx.core.content.ContextCompat.getSystemService
import com.mythicpalette.andropen.data.EventType
import com.mythicpalette.andropen.data.PointerInfo
import com.mythicpalette.andropen.data.PointerType
import com.mythicpalette.andropen.data.serialize
import com.mythicpalette.andropen.helpers.SocketHandler
import java.nio.ByteBuffer
import java.nio.ByteOrder

/**
 * TODO: document your custom view class.
 */
class TouchInputView : Button {
    private var penHovering: Boolean = false
    private var penTouching: Boolean = false

    var socketHandler: SocketHandler? = null


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

        // Draw the border
        val padding = borderPaint.strokeWidth / 2
//        canvas.drawRect(
//            padding,
//            padding,
//            width - padding,
//            height - padding,
//            borderPaint
//        )
        val lineLength = this.width*0.1f
        val cornerX: Float = this.width.toFloat()
        val cornerY: Float = this.height.toFloat()

        // Top left corner
        canvas.drawLine(0f, 0f, lineLength, 0f, borderPaint)
        canvas.drawLine(0f, 0f, 0f, lineLength, borderPaint)

        // Top right corner
        canvas.drawLine(cornerX-lineLength, 0f, cornerX, 0f, borderPaint)
        canvas.drawLine(cornerX, 0f, cornerX, lineLength, borderPaint)

        // Bottom left corner
        canvas.drawLine(0f, cornerY - lineLength, 0f, cornerY, borderPaint);
        canvas.drawLine(0f, cornerY, lineLength, cornerY, borderPaint);

        // Bottom right corner
        canvas.drawLine(cornerX, cornerY - lineLength, cornerX, cornerY, borderPaint);
        canvas.drawLine(cornerX - lineLength, cornerY, cornerX, cornerY, borderPaint);
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
                    if ( penHovering || penTouching ) continue

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

        // Create the byte buffer.
        val buffer =
            ByteBuffer.allocate(4 + (68 * infos.size)).order(ByteOrder.LITTLE_ENDIAN)

        // Serialize the number of touch events to send.
        buffer.putInt(infos.size)

        // Serialize the touch data.
        for (pi in infos)
            buffer.put(pi.serialize())

        // Send the data.
        socketHandler?.send(buffer.array())
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

        // Send the pointer count.
        val buffer = ByteBuffer.allocate(4 + (68*ev.pointerCount)).order(ByteOrder.LITTLE_ENDIAN)
        buffer.putInt(ev.pointerCount)

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
        buffer.put(pi.serialize())

        socketHandler?.send(buffer.array())
        return true
    }

    fun clearCanvas() {
        path.reset()
        invalidate() // Redraw the view
    }
}