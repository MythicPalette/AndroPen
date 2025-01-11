package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import com.mythicpalette.andropen.R
import com.mythicpalette.andropen.data.PointerInfo
import com.mythicpalette.andropen.data.toPointerInfo
import com.mythicpalette.andropen.helpers.NativeInput
import com.mythicpalette.andropen.helpers.Settings
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

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
    private var penHover: Boolean = false
    private var penDown: Boolean = false
    private var touchDown: Boolean = false

    var onTouch: (Int, MutableList<PointerInfo>) -> Unit = {_, _ ->}
    var onHover: (Int, PointerInfo) -> Unit = {_, _ ->}

    var borderCoverStyle: BorderCoverStyle = BorderCoverStyle.Shortest
    var borderCoverage: Float = 1f

    var SenderId: Int = 0

    private var lastTouches: MutableList<PointerInfo> = mutableListOf()

    constructor(context: Context, attrs: AttributeSet) : super(context, attrs) {
        init(attrs)
    }

    constructor(context: Context, attrs: AttributeSet, defStyle: Int) : super(
        context,
        attrs,
        defStyle
    ) {
        init(attrs)
    }

    private fun init(attrs: AttributeSet) {
        // Read custom attributes
        context.theme.obtainStyledAttributes(
            attrs,
            R.styleable.BaseTouchView,
            0, 0
        ).apply {
            try {
                SenderId = getInt(R.styleable.BaseTouchView_senderId, 0)
                val strokeWidth = getDimension(R.styleable.BaseTouchView_strokeWidth, 4f)
                val borderColor = getColor(R.styleable.BaseTouchView_borderColor, Color.WHITE)
                borderCoverage = getFloat(R.styleable.BaseTouchView_borderCoverage, 1f)
                borderCoverStyle = BorderCoverStyle.fromValue(getInt(R.styleable.BaseTouchView_borderCoverageStyle, 0))

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

    //private val path = android.graphics.Path()

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
        println(NativeInput.getPointerInfo())
        val infos: MutableList<PointerInfo> = mutableListOf()
        val action = ev.actionMasked
        val stamp = System.currentTimeMillis()
        val viewWidth = this.width
        val viewHeight = this.height

        // Iterate over all pointers to handle pen input (pressure, movement, etc.)
        for (i in 0 until ev.pointerCount) {
            val pi = ev.toPointerInfo(i, stamp, viewWidth, viewHeight )

            // Check if the pointer has a previous track
            for( prev in lastTouches ) {
                if ( prev.pointerId == pi.pointerId ) { // Found a match
                    // Get the difference between locations.
                    pi.velocityX = pi.x - prev.x
                    pi.velocityY = pi.y - prev.y
                }
            }

            // Check if the input device is a pen or a finger
            when (pi.pointerType) {
                MotionEvent.TOOL_TYPE_STYLUS -> {
                    when (action) {
                        MotionEvent.ACTION_DOWN -> penDown = true
                        MotionEvent.ACTION_POINTER_DOWN -> penDown = true
                        MotionEvent.ACTION_UP -> penDown = false
                        MotionEvent.ACTION_POINTER_UP -> penDown = false
                    }
                }

                MotionEvent.TOOL_TYPE_FINGER -> {
                    // If the pen is in use and the event is not UP or Pointer UP then ignore it
                    if (
                        Settings.PenBlocksTouch
                        && ( penHover || penDown )
                        && (action != MotionEvent.ACTION_UP && action != MotionEvent.ACTION_POINTER_UP))
                        continue

                    // Finger touch detected (tap or gesture)
                    when (action) {
                        MotionEvent.ACTION_DOWN -> touchDown = true
                        MotionEvent.ACTION_POINTER_DOWN -> touchDown = true
                        MotionEvent.ACTION_UP -> touchDown = false
                        MotionEvent.ACTION_POINTER_UP -> touchDown = false
                    }
                }
                else -> {}
            }
            infos.add(pi)
        }
        this.lastTouches = infos
        this.onTouch(this.SenderId, infos)
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
        val pi = ev.toPointerInfo(0, System.currentTimeMillis(), this.width, this.height )

        when (pi.eventType) {
            MotionEvent.ACTION_HOVER_ENTER -> penHover = true
            MotionEvent.ACTION_HOVER_EXIT -> {
                /*
                 Run the HOVER_EXIT event on a delay to prevent sending "HOVER_EXIT" event before
                 the pen down event. This is because the event chain for Windows is different
                 from the Android event chain.
                 */
                CoroutineScope(Dispatchers.IO).launch{
                    delay(500)
                    if ( penDown ) return@launch

                    // Upon reaching this code, the pen has left range.
                    penHover = false
                    lastTouches.clear()
                    lastTouches.add(pi)
                    onHover(SenderId, pi);
                }

                // Return from here to prevent following the normal flow.
                return true
            }
        }

        this.lastTouches.clear()
        this.lastTouches.add(pi)
        this.onHover(this.SenderId, pi);
        return true
    }

    fun clearCanvas() {
        //path.reset()
        invalidate() // Redraw the view
    }
}