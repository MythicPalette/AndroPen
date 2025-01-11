package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.graphics.Point
import android.graphics.PointF
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import androidx.compose.foundation.gestures.Orientation
import com.mythicpalette.andropen.R
import com.mythicpalette.andropen.data.EventType
import com.mythicpalette.andropen.data.PointerInfo
import com.mythicpalette.andropen.data.PointerType
import com.mythicpalette.andropen.helpers.Settings
import kotlin.math.abs

class TouchSliderView : View {
    var onTouch: (Int, PointerInfo) -> Unit = {_, _ ->}

    var borderCoverStyle: BorderCoverStyle = BorderCoverStyle.Shortest
    var borderCoverage: Float = 1f

    var SenderId: Int = 0

    private var eventPoint: PointF? = null

    /*
     Move Sensitivity is used to control how frequently move events passed. This is
     to ensure that touch input views used like sliders
     */
    var sensitivity: Float = 0.1f

    private var orientation: Orientation = Orientation.Vertical

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

        // Read custom attributes
        context.theme.obtainStyledAttributes(
            attrs,
            R.styleable.TouchSliderView,
            0, 0
        ).apply {
            try {
                sensitivity = getFloat(R.styleable.TouchSliderView_sensitivity, 0.1f)
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
        val action = ev.actionMasked
        val stamp = System.currentTimeMillis()
        val viewWidth = this.width
        val viewHeight = this.height
        val evPoint = this.eventPoint

        val pi = PointerInfo(
            pointerId = ev.getPointerId(0), // Pointer ID
            x = ev.x, y = ev.y,                        // Coordinates
            timeStamp = stamp,                         // Timestamp
            viewWidth = viewWidth,                     // View resolution
            viewHeight = viewHeight,
        )

        // Get the action type and pass it to the pointer info.
        when (action) {
            MotionEvent.ACTION_DOWN -> {
                pi.eventType = EventType.DOWN
                eventPoint = PointF(ev.x, ev.y)
            }     // Event Type
            MotionEvent.ACTION_POINTER_DOWN -> pi.eventType = EventType.POINTER_DOWN
            MotionEvent.ACTION_UP -> {
                pi.eventType = EventType.UP
                eventPoint = null
            }
            MotionEvent.ACTION_POINTER_UP -> {
                pi.eventType = EventType.POINTER_UP
                eventPoint = null
            }
            MotionEvent.ACTION_MOVE -> {
                if ( this.orientation == Orientation.Vertical ) {
                    if ( evPoint == null ) return true

                    // Verify we have travelled the requisite vertical distance for the sensitivity
                    if ( abs(evPoint.y - ev.y) < (this.height * this.sensitivity) )
                        return true // Return true to prevent sending

                    /*
                        If the sensitivity threshold has been met, we want to set the new point as
                        the event point so the next move will be compared to this point.
                     */
                    pi.velocityY = if( ev.y > evPoint.y ) 1f else -1f
                    this.eventPoint = PointF(ev.x, ev.y)
                }
            }
        }

        this.onTouch(this.SenderId, pi)
        return true
    }

    fun clearCanvas() {
        //path.reset()
        invalidate() // Redraw the view
    }
}