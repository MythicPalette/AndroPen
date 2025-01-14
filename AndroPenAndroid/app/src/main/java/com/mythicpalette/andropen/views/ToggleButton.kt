package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.util.AttributeSet

open class ToggleButton : androidx.appcompat.widget.AppCompatButton {
    var buttonOn: Boolean = false
        set(value) {
            field = value
            invalidate();
        }

    protected var _barStep: Int = 0
    protected var _barWidth: Int = 0
    protected var _barHeight: Int = 0

    protected val _whitePaint = Paint().apply {
        color = Color.WHITE  // Border color
        style = Paint.Style.FILL
    }

    protected val _grayPaint = Paint().apply {
        color = Color.DKGRAY  // Border color
        style = Paint.Style.FILL
    }

    protected val _redPaint = Paint().apply {
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
        this._barStep = (this.width * 0.25).toInt()
        this._barWidth = (this.width * 0.2).toInt()
        this._barHeight = (this.height * 0.25).toInt()
    }
    override fun onDraw(canvas: Canvas) {
        drawIcon(canvas)

        if ( !this.buttonOn ) {
            canvas.drawLine(0f, 0f, _barStep*2f, _barStep*2f, _redPaint)
            canvas.drawLine(_barStep*2f, 0f, 0f, _barStep*2f, _redPaint)
        }
    }

    open fun drawIcon(canvas: Canvas) {

    }
}