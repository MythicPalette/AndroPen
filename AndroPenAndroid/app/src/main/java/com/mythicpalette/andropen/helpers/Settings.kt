package com.mythicpalette.andropen.helpers

import android.content.Context
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity.MODE_PRIVATE

object Settings {
    private const val PREF_NAME = "MyAppPreferences"

    const val IP_ADDRESS = "ip_address"
    const val PORT_ADDRESS = "port_address"
    const val PEN_BLOCKS_TOUCH = "pen_blocks_touch"
    const val TOUCH_DISABLED = "touch_disabled"

    public var IpAddress: String = "0.0.0.0"
        private set
    public var Port: Int = 18998
        private set
    public var PenBlocksTouch: Boolean = true
        private set
    public var TouchDisabled: Boolean = false
        private set

    fun init(context: Context) {
        IpAddress = getSharedPreferences(context).getString(IP_ADDRESS, "0.0.0.0") ?: "0.0.0.0"
        Port = getSharedPreferences(context).getInt(PORT_ADDRESS, 0)
        PenBlocksTouch = getSharedPreferences(context).getBoolean(PEN_BLOCKS_TOUCH, false)
        TouchDisabled = getSharedPreferences(context).getBoolean(TOUCH_DISABLED, false)
    }

    private fun getSharedPreferences(context: Context): SharedPreferences {
        return context.getSharedPreferences(PREF_NAME, MODE_PRIVATE)
    }
    fun setIpAddress(context: Context, ip: String) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putString(IP_ADDRESS, ip)
        editor.apply()
        IpAddress = ip
    }
    fun setPort(context: Context, port: Int) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putInt(PORT_ADDRESS, port)
        editor.apply()
        Port = port
    }
    fun setPenBlocksTouch(context: Context, touch: Boolean) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putBoolean(PEN_BLOCKS_TOUCH, touch)
        editor.apply()
        PenBlocksTouch = touch
    }
    fun setTouchDisabled(context: Context, touch: Boolean) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putBoolean(TOUCH_DISABLED, touch)
        editor.apply()
        TouchDisabled = touch
    }
    fun set(context: Context, ip: String? = null, port: Int? = null, penTouch: Boolean? = null, touchDisabled: Boolean? = null) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()

        if ( ip != null )
        {
            editor.putString(IP_ADDRESS, ip)
            IpAddress = ip
        }

        if ( port != null )
        {
            editor.putInt(PORT_ADDRESS, port)
            Port = port
        }

        if ( penTouch != null )
        {
            editor.putBoolean(PEN_BLOCKS_TOUCH, penTouch)
            PenBlocksTouch = penTouch
        }

        if ( touchDisabled != null )
        {
            editor.putBoolean(TOUCH_DISABLED, touchDisabled)
            TouchDisabled = touchDisabled
        }

        editor.apply()
    }
}