package com.mythicpalette.andropen.views

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint
import android.graphics.Path
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import com.mythicpalette.andropen.R
import com.mythicpalette.andropen.data.PointerInfo
import com.mythicpalette.andropen.data.toPointerInfo
import com.mythicpalette.andropen.helpers.Settings
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

enum class BorderCoverStyle(val value: Int) {
    Shortest(0), Longest(1), Independent(2);

    companion object {
        fun fromValue(value: Int): BorderCoverStyle {
            return entries.find { it.value == value }
                ?: throw IllegalArgumentException("Invalid value for BorderStyle: $value")
        }
    }
}

/*
 * The TouchInputView takes touch and pen input and turns it into transmittable data for
 * sending to the companion program on Windows.
 */
class TouchInputView : View {
    private var _penHover: Boolean = false
    private var _penDown: Boolean = false
    private var _touchDown: Boolean = false

    var onTouch: (Int, MutableList<PointerInfo>) -> Unit = {_, _ ->}
    var onHover: (Int, PointerInfo) -> Unit = {_, _ ->}

    private var _borderColor: Int = Color.WHITE
    private var _borderCoverStyle: BorderCoverStyle = BorderCoverStyle.Shortest
    private var _borderCoverage: Float = 1f

    var SenderId: Int = 0

    private var _lastTouches: MutableList<PointerInfo> = mutableListOf()

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
            R.styleable.TouchInputView,
            0, 0
        ).apply {
            try {
                SenderId = getInt(R.styleable.TouchInputView_senderId, 0)
                val strokeWidth = getDimension(R.styleable.TouchInputView_strokeWidth, 4f)
                _borderColor = getColor(R.styleable.TouchInputView_borderColor, Color.WHITE)
                _borderCoverage = getFloat(R.styleable.TouchInputView_borderCoverage, 1f)
                _borderCoverStyle = BorderCoverStyle.fromValue(getInt(R.styleable.TouchInputView_borderCoverageStyle, 0))

                borderPaint.apply {
                    color = _borderColor
                    this.strokeWidth = strokeWidth
                    style = Paint.Style.STROKE
                }

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


    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)

        val cornerX: Float = this.width.toFloat()
        val cornerY: Float = this.height.toFloat()

        if ( this._borderCoverage == 0f )
            return

        var vLen = this.height*(this._borderCoverage * 0.5f)
        var hLen = this.width*(this._borderCoverage * 0.5f)

        if ( this._borderCoverStyle == BorderCoverStyle.Shortest )
        {
            // If we're using the shortest side then find the shorter value and duplicate
            if ( vLen < hLen )
                hLen = vLen
            else
                vLen = hLen
        }
        else if ( this._borderCoverStyle == BorderCoverStyle.Longest )
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
        val infos: MutableList<PointerInfo> = mutableListOf()
        val action = ev.actionMasked
        val stamp = System.currentTimeMillis()
        val viewWidth = this.width
        val viewHeight = this.height

        // Iterate over all pointers to handle pen input (pressure, movement, etc.)
        for (i in 0 until ev.pointerCount) {
            val pi = ev.toPointerInfo(i, stamp, viewWidth, viewHeight )

            // Check if the pointer has a previous track
            for( prev in _lastTouches ) {
                if ( prev.pointerId == pi.pointerId ) { // Found a match
                    // Get the difference between locations.
                    pi.velocityX = pi.x - prev.x
                    pi.velocityY = pi.y - prev.y
                }
            }

            // Check if the input device is a pen or a finger
            when (pi.pointerType) {
                MotionEvent.TOOL_TYPE_STYLUS -> {
                    when (action) {
                        MotionEvent.ACTION_DOWN,
                        MotionEvent.ACTION_POINTER_DOWN -> _penDown = true
                        MotionEvent.ACTION_UP,
                        MotionEvent.ACTION_POINTER_UP -> _penDown = false
                    }
                }

                MotionEvent.TOOL_TYPE_FINGER -> {
                    // If touch is completely disabled, don't allow finger touches.
                    if ( Settings.TouchDisabled )
                        return true

                    // If the pen is in use and the event is not UP or Pointer UP then ignore it
                    if (
                        Settings.PenBlocksTouch
                        && ( _penHover || _penDown )
                        && (action != MotionEvent.ACTION_UP && action != MotionEvent.ACTION_POINTER_UP))
                        continue

                    // Finger touch detected (tap or gesture)
                    when (action) {
                        MotionEvent.ACTION_DOWN -> _touchDown = true
                        MotionEvent.ACTION_POINTER_DOWN -> _touchDown = true
                        MotionEvent.ACTION_UP -> _touchDown = false
                        MotionEvent.ACTION_POINTER_UP -> _touchDown = false
                    }
                }
                else -> {}
            }
            infos.add(pi)
        }
        this._lastTouches = infos
        this.onTouch(this.SenderId, infos)
        return true
    }

    override fun onHoverEvent(ev: MotionEvent): Boolean {
        val pi = ev.toPointerInfo(0, System.currentTimeMillis(), this.width, this.height )

        when (pi.eventType) {
            MotionEvent.ACTION_HOVER_ENTER -> _penHover = true
            MotionEvent.ACTION_HOVER_EXIT -> {
                /*
                 Run the HOVER_EXIT event on a delay to prevent sending "HOVER_EXIT" event before
                 the pen down event. This is because the event chain for Windows is different
                 from the Android event chain.
                 */
                CoroutineScope(Dispatchers.IO).launch{
                    delay(500)
                    if ( _penDown ) return@launch

                    // Upon reaching this code, the pen has left range.
                    _penHover = false
                    _lastTouches.clear()
                    _lastTouches.add(pi)
                    onHover(SenderId, pi);
                }

                // Return from here to prevent following the normal flow.
                return true
            }
        }

        this._lastTouches.clear()
        this._lastTouches.add(pi)
        this.onHover(this.SenderId, pi);
        return true
    }
}