package com.mythicpalette.andropen.helpers

import android.content.Context
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.IOException
import java.io.InputStream
import java.io.OutputStream
import java.net.Socket

data class ConnectionState (
    var connected: Boolean,
    var host: String,
    var port: Int
)

interface SocketStateListener {
    fun onConnectionStateChanged(state: ConnectionState);
}
class SocketHandler {
    private var _socket: Socket? = null
    private var _inputStream: InputStream? = null
    private var _outputStream: OutputStream? = null

    // EVENT HANDLING
    private val _listeners = mutableListOf<SocketStateListener>();

    fun addListener(listener: SocketStateListener) {
        _listeners.add(listener);
    }

    private fun notifyStateChanged(state: ConnectionState) {
        for ( listener in _listeners ) {
            listener.onConnectionStateChanged(state);
        }
    }

    // Step 1: Function to connect to the socket server
    internal fun connectToSocket(context: Context) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val host = Settings.IpAddress

                // Don't attempt to connect if the IP address isn't set.
                if ( host == "0.0.0.0") return@launch

                val port = Settings.Port
                _socket = Socket(host, port)

                _inputStream = _socket?.getInputStream()
                _outputStream = _socket?.getOutputStream()

                monitorSocket {
                    notifyStateChanged(ConnectionState(false, host, port))
                }
                println("Connected to the server at $host:$port")

                notifyStateChanged(ConnectionState(true, host, port));
                // Start listening for server messages in the background
                listenForServerMessages(host, port)
            } catch (e: IOException) {
                e.printStackTrace()
            }
        }
    }

    // Step 2: Function to listen for incoming messages from the server
    private suspend fun listenForServerMessages(host: String, port: Int) {
        withContext(Dispatchers.IO) {
            try {
                val buffer = ByteArray(1024)
                var bytesRead: Int

                while (_socket?.isConnected == true) {
                    // Reading from the server
                    bytesRead = _inputStream?.read(buffer) ?: -1
                    if (bytesRead != -1) {
                        val response = String(buffer, 0, bytesRead)
                        println("Received from server: $response")
                    } else {
                        break;
                    }
                }
            } catch (e: IOException) {
                e.printStackTrace()
                notifyStateChanged(ConnectionState(false, host, port));
                //showToast("Connection lost")
            }
        }
    }

    internal fun send(bytes: ByteArray) {
        if ( _outputStream == null ) return;

        CoroutineScope(Dispatchers.IO).launch {
            try {
                _outputStream?.write(bytes);
                _outputStream?.flush();
            } catch (e: IOException) {
                e.printStackTrace()
                //showToast("Error sending bytes")
            }
        }
    }

    private fun monitorSocket(onConnectionLost: () -> Unit) {
        Thread {
            while (true) {
                if (_socket?.isConnected != true) {
                    println("Connection lost!")
                    break
                }
                Thread.sleep(5000) // Check connection every 5 seconds
            }
            onConnectionLost();
        }.start()
    }
}