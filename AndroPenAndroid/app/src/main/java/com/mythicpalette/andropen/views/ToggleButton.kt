package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.graphics.Rect
import android.graphics.drawable.Drawable
import android.text.TextPaint
import android.util.AttributeSet
import android.view.View
import android.widget.Button
import androidx.compose.material3.Button
import com.mythicpalette.andropen.R

/**
 * TODO: document your custom view class.
 */
open class ToggleButton : androidx.appcompat.widget.AppCompatButton {

    /**
     * The text to draw
     */
    var buttonOn: Boolean = false
        set(value) {
            field = value
            invalidate();
        }

    protected var barStep: Int = 0
    protected var barWidth: Int = 0
    protected var barHeight: Int = 0

    protected val whitePaint = Paint().apply {
        color = Color.WHITE  // Border color
        style = Paint.Style.FILL
    }
    protected val grayPaint = Paint().apply {
        color = Color.DKGRAY  // Border color
        style = Paint.Style.FILL
    }
    protected val redPaint = Paint().apply {
        color = Color.RED
        strokeWidth = 3f;
        style = Paint.Style.STROKE
    }

    constructor(context: Context) : super(context) {
        init()
    }

    constructor(context: Context, attrs: AttributeSet) : super(context, attrs) {
        init()
    }

    constructor(context: Context, attrs: AttributeSet, defStyle: Int) : super(
        context,
        attrs,
        defStyle
    ) {
        init()
    }

    private fun init() {
        this.setBackgroundColor(Color.TRANSPARENT)
    }

    override fun onSizeChanged(w: Int, h: Int, oldw: Int, oldh: Int) {
        super.onSizeChanged(w, h, oldw, oldh)
        // Initialize the dimensions
        this.barStep = (this.width * 0.25).toInt()
        this.barWidth = (this.width * 0.2).toInt()
        this.barHeight = (this.height * 0.25).toInt()
    }
    override fun onDraw(canvas: Canvas) {
        drawIcon(canvas)

        if ( !this.buttonOn ) {
            canvas.drawLine(0f, 0f, barStep*2f, barStep*2f, redPaint)
            canvas.drawLine(barStep*2f, 0f, 0f, barStep*2f, redPaint)
        }
    }

    open fun drawIcon(canvas: Canvas) {

    }
}