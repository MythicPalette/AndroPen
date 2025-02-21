package com.mythicpalette.andropen.helpers

import android.content.Context
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity.MODE_PRIVATE

interface SettingsEventListener {
    fun onSettingsChanged()
}
object Settings {
    private const val PREF_NAME = "MyAppPreferences"

    private val _listeners = mutableListOf<SettingsEventListener>()

    const val IP_ADDRESS = "ip_address"
    const val PORT_ADDRESS = "port_address"
    const val PEN_BLOCKS_TOUCH = "pen_blocks_touch"
    const val TOUCH_DISABLED = "touch_disabled"
    const val SLIDER_1_SENSITIVITY = "slider_1_sensitivity"
    const val SLIDER_2_SENSITIVITY = "slider_2_sensitivity"

    var IpAddress: String = "0.0.0.0"
        private set
    var Port: Int = 18998
        private set
    var PenBlocksTouch: Boolean = true
        private set
    var TouchDisabled: Boolean = false
        private set
    var Slider1Sensitivity: Float = 0.1f
        private set
    var Slider2Sensitivity: Float = 0.1f
        private set

    fun init(context: Context) {
        IpAddress = getSharedPreferences(context).getString(IP_ADDRESS, "0.0.0.0") ?: "0.0.0.0"
        Port = getSharedPreferences(context).getInt(PORT_ADDRESS, 0)
        PenBlocksTouch = getSharedPreferences(context).getBoolean(PEN_BLOCKS_TOUCH, false)
        TouchDisabled = getSharedPreferences(context).getBoolean(TOUCH_DISABLED, false)
        Slider1Sensitivity = getSharedPreferences(context).getFloat(SLIDER_1_SENSITIVITY, 0.1f)
        Slider2Sensitivity = getSharedPreferences(context).getFloat(SLIDER_2_SENSITIVITY, 0.1f)
    }

    fun addListener(listener: SettingsEventListener) {
        _listeners.add(listener)
    }
    fun alertChange() {
        for ( l in _listeners )
            l.onSettingsChanged()
    }

    private fun getSharedPreferences(context: Context): SharedPreferences {
        return context.getSharedPreferences(PREF_NAME, MODE_PRIVATE)
    }
    fun setTouchDisabled(context: Context, touch: Boolean) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putBoolean(TOUCH_DISABLED, touch)
        editor.apply()
        TouchDisabled = touch
        alertChange()
    }
    fun set(
        context: Context,
        ip: String? = null,
        port: Int? = null,
        penTouch: Boolean? = null,
        touchDisabled: Boolean? = null,
        slider1Sensitivity: Float? = null,
        slider2Sensitivity: Float? = null
    ) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()

        if ( ip != null )
        {
            editor.putString(IP_ADDRESS, ip)
            this.IpAddress = ip
        }

        if ( port != null )
        {
            editor.putInt(PORT_ADDRESS, port)
            this.Port = port
        }

        if ( penTouch != null )
        {
            editor.putBoolean(PEN_BLOCKS_TOUCH, penTouch)
            this.PenBlocksTouch = penTouch
        }

        if ( touchDisabled != null )
        {
            editor.putBoolean(TOUCH_DISABLED, touchDisabled)
            this.TouchDisabled = touchDisabled
        }

        if ( slider1Sensitivity != null )
        {
            editor.putFloat(SLIDER_1_SENSITIVITY, slider1Sensitivity)
            this.Slider1Sensitivity = slider1Sensitivity
        }

        if ( slider2Sensitivity != null )
        {
            editor.putFloat(SLIDER_1_SENSITIVITY, slider2Sensitivity)
            this.Slider2Sensitivity = slider2Sensitivity
        }

        alertChange()
        editor.apply()
    }
}