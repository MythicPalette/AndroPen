package com.mythicpalette.andropen

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.graphics.Rect
import android.util.AttributeSet
import android.widget.Button

/**
 * TODO: document your custom view class.
 */
class SignalButton : Button {
    private var barStep: Int = 0
    private var barWidth: Int = 0
    private var barHeight: Int = 0
    var signalBars: Array<Rect?> = arrayOfNulls(4)

    var connected: Boolean = false
        set(value) {
            field = value
            invalidate();
        }

    private val connectedPaint = Paint().apply {
        color = Color.WHITE  // Border color
        style = Paint.Style.FILL
    }
    private val disconnectedPaint = Paint().apply {
        color = Color.DKGRAY  // Border color
        style = Paint.Style.FILL
    }
    private val redPaint = Paint().apply {
        color = Color.RED
        strokeWidth = 3f;
        style = Paint.Style.STROKE
    }

    constructor(context: Context) : super(context) {
        init(null, 0)
    }

    constructor(context: Context, attrs: AttributeSet) : super(context, attrs) {
        init(attrs, 0)
    }

    constructor(context: Context, attrs: AttributeSet, defStyle: Int) : super(
        context,
        attrs,
        defStyle
    ) {
        init(attrs, defStyle)
    }

    private fun init(attrs: AttributeSet?, defStyle: Int) {
        // Load attributes
        val a = context.obtainStyledAttributes(
            attrs, R.styleable.SignalButton, defStyle, 0
        )
        a.recycle()
        setBackgroundColor(Color.TRANSPARENT)
    }

    override fun onSizeChanged(w: Int, h: Int, oldw: Int, oldh: Int) {
        super.onSizeChanged(w, h, oldw, oldh)
        // Initialize the dimensions
        this.barStep = (this.width * 0.25).toInt()
        this.barWidth = (this.width * 0.2).toInt()
        this.barHeight = (this.height * 0.25).toInt()

        // Assign values to signalBars
        signalBars[0] = Rect(0, barHeight * 3, barWidth, this.height)
        signalBars[1] = Rect(barStep, barHeight * 2, barStep + barWidth, this.height)
        signalBars[2] = Rect(barStep*2, barHeight, barStep*2 + barWidth, this.height)
        signalBars[3] = Rect(barStep*3, 0, this.width, this.height)
    }

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)

        for ( bar in this.signalBars ) {
            bar?.let {
                canvas.drawRect(
                    it,
                    if (this.connected) connectedPaint else disconnectedPaint
                )
            }
        }
        if ( !this.connected ) {
            canvas.drawLine(0f, 0f, barStep*2f, barStep*2f, redPaint)
            canvas.drawLine(barStep*2f, 0f, 0f, barStep*2f, redPaint)
        }
    }
}