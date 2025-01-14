package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Rect
import android.util.AttributeSet

/**
 * TODO: document your custom view class.
 */
class SignalButton : ToggleButton {
    private var signalBars: Array<Rect?> = arrayOfNulls(4)

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
        signalBars[0] = Rect(0, _barHeight * 3, _barWidth, this.height)
        signalBars[1] = Rect(_barStep, _barHeight * 2, _barStep + _barWidth, this.height)
        signalBars[2] = Rect(_barStep*2, _barHeight, _barStep*2 + _barWidth, this.height)
        signalBars[3] = Rect(_barStep*3, 0, this.width, this.height)
    }

    override fun drawIcon(canvas: Canvas) {
        for ( bar in this.signalBars ) {
            bar?.let {
                canvas.drawRect(
                    it,
                    if (this.buttonOn) _whitePaint else _grayPaint
                )
            }
        }
    }
}