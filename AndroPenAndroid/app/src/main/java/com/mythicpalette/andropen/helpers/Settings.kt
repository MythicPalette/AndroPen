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

    private fun getSharedPreferences(context: Context): SharedPreferences {
        return context.getSharedPreferences(PREF_NAME, MODE_PRIVATE)
    }
    fun getIpAddress(context: Context): String {
        return getSharedPreferences(context).getString(IP_ADDRESS, "0.0.0.0") ?: "0.0.0.0"
    }
    fun setIpAddress(context: Context, ip: String) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putString(IP_ADDRESS, ip)
        editor.apply()
    }

    fun getPort(context: Context): Int {
        val result = getSharedPreferences(context).getInt(PORT_ADDRESS, 0)
        return result
    }
    fun setPort(context: Context, port: Int) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putInt(PORT_ADDRESS, port)
        editor.apply()
    }

    fun getPenBlocksTouch(context: Context): Boolean {
        val result =  getSharedPreferences(context).getBoolean(PEN_BLOCKS_TOUCH, false)
        return result
    }
    fun setPenBlocksTouch(context: Context, touch: Boolean) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putBoolean(PEN_BLOCKS_TOUCH, touch)
        editor.apply()
    }

    fun getTouchDisabled(context: Context): Boolean {
        return getSharedPreferences(context).getBoolean(TOUCH_DISABLED, false)
    }
    fun setIpAddress(context: Context, touch: Boolean) {
        val pref = getSharedPreferences(context)
        val editor = pref.edit()
        editor.putBoolean(TOUCH_DISABLED, touch)
        editor.apply()
    }
}