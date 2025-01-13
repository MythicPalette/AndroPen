package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.util.AttributeSet
import android.view.View
import com.mythicpalette.andropen.R

class ExpressKey @JvmOverloads constructor(
    context: Context, attrs: AttributeSet? = null
) : View(context, attrs) {
    var SenderId: Int = 0


    private fun init(attrs: AttributeSet) {
        // Read custom attributes
        context.theme.obtainStyledAttributes(
            attrs,
            R.styleable.BaseTouchView,
            0, 0
        ).apply {
            try {
                SenderId = getInt(R.styleable.BaseTouchView_senderId, 0)
            } finally {
                recycle()
            }
        }
    }

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