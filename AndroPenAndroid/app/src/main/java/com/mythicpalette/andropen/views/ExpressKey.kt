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
import com.mythicpalette.andropen.helpers.Settings

class ExpressKey @JvmOverloads constructor(
    context: Context, attrs: AttributeSet? = null
) : View(context, attrs) {
    var onTouch: (Int, PointerInfo) -> Unit = { _, _ ->}

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

    override fun onTouchEvent(ev: MotionEvent): Boolean {
        val stamp = System.currentTimeMillis()
        val viewWidth = this.width
        val viewHeight = this.height

        // Iterate over all pointers to handle pen input (pressure, movement, etc.)
        val pi = ev.toPointerInfo(0, stamp, viewWidth, viewHeight )

        this.onTouch(this.SenderId, pi)
        return true
    }
}