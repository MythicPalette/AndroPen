package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.util.AttributeSet

class TouchToggleButton : ToggleButton {

    constructor(context: Context) : super(context)
    constructor(context: Context, attrs: AttributeSet) : super(context, attrs)
    constructor(context: Context, attrs: AttributeSet, defStyle: Int) : super(
        context,
        attrs,
        defStyle
    )

    var arcPaint = Paint().apply {
        color = Color.RED
        strokeWidth = 2f;
        style = Paint.Style.STROKE
    }

    override fun drawIcon(canvas: Canvas) {
        val centerX = width / 2f
        val centerY = height / 2f
        val baseRadius = width / 5f // Proportional scaling for the inner circle

        val paint = if( this.buttonOn ) _whitePaint else _grayPaint
        arcPaint.color = paint.color

        // Draw the main circle
        canvas.drawCircle(centerX, centerY, baseRadius, paint)

        // Draw concentric arcs around the circle
        val arcSpacing = baseRadius / 2f
        for (i in 1..3) {
            canvas.drawArc(
                centerX - baseRadius - i * arcSpacing,
                centerY - baseRadius - i * arcSpacing,
                centerX + baseRadius + i * arcSpacing,
                centerY + baseRadius + i * arcSpacing,
                105f,
                330f, // Slightly reduce the arc's angle
                false,
                arcPaint
            )
        }
    }
}