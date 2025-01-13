package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.graphics.Rect
import android.util.AttributeSet
import android.widget.Button
import com.mythicpalette.andropen.R

/**
 * TODO: document your custom view class.
 */
class SignalButton : ToggleButton {
    var signalBars: Array<Rect?> = arrayOfNulls(4)

    constructor(context: Context) : super(context) {
    }

    constructor(context: Context, attrs: AttributeSet) : super(context, attrs) {
    }

    constructor(context: Context, attrs: AttributeSet, defStyle: Int) : super(
        context,
        attrs,
        defStyle
    ) {
    }

    override fun onSizeChanged(w: Int, h: Int, oldw: Int, oldh: Int) {
        super.onSizeChanged(w, h, oldw, oldh)

        // Assign values to signalBars
        signalBars[0] = Rect(0, barHeight * 3, barWidth, this.height)
        signalBars[1] = Rect(barStep, barHeight * 2, barStep + barWidth, this.height)
        signalBars[2] = Rect(barStep*2, barHeight, barStep*2 + barWidth, this.height)
        signalBars[3] = Rect(barStep*3, 0, this.width, this.height)
    }

    override fun drawIcon(canvas: Canvas) {
        for ( bar in this.signalBars ) {
            bar?.let {
                canvas.drawRect(
                    it,
                    if (this.buttonOn) whitePaint else grayPaint
                )
            }
        }
    }
}