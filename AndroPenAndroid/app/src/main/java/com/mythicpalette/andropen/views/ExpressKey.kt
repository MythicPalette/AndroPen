package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.util.AttributeSet
import android.view.View

class ExpressKey @JvmOverloads constructor(
    context: Context, attrs: AttributeSet? = null
) : View(context, attrs) {

    protected val borderPaint = Paint().apply {
        color = Color.WHITE
        strokeWidth = 3f;
        style = Paint.Style.STROKE
    }

    override fun onDraw(canvas: Canvas) {
        canvas.drawRect(
            0f, 0f, this.width.toFloat(), this.height.toFloat(),
            borderPaint
        )
    }
}