package com.mythicpalette.andropen.helpers

import com.mythicpalette.andropen.data.PointerInfo
import com.mythicpalette.andropen.data.serialize
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.IOException
import java.io.InputStream
import java.io.OutputStream
import java.net.Socket
import java.nio.ByteBuffer
import java.nio.ByteOrder

data class ConnectionState (
    var Connected: Boolean,
    var Host: String,
    var Port: Int
)

interface SocketStateListener {
    fun onConnectionStateChanged(state: ConnectionState);
}
class SocketHandler(private var host: String, private var port: Int) {
    private var socket: Socket? = null
    private var inputStream: InputStream? = null
    private var outputStream: OutputStream? = null

    private var PACKET_SIGNATURE = intArrayOf(25613, 16513, 13461, 13414);

    // EVENT HANDLING
    private val listeners = mutableListOf<SocketStateListener>();
    fun addListener(listener: SocketStateListener) {
        listeners.add(listener);
    }

    fun removeListener(listener: SocketStateListener) {
        listeners.remove(listener);
    }

    private fun notifyStateChanged(state: ConnectionState) {
        for ( listener in listeners ) {
            listener.onConnectionStateChanged(state);
        }
    }

    // Step 1: Function to connect to the socket server
    internal fun connectToSocket() {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                println("Connected to the server at $host:$port")
                socket = Socket(host, port);

                inputStream = socket?.getInputStream()
                outputStream = socket?.getOutputStream()

                monitorSocket {
                    notifyStateChanged(ConnectionState(false, host, port))
                }

                notifyStateChanged(ConnectionState(true, host, port));
                // Start listening for server messages in the background
                listenForServerMessages()
            } catch (e: IOException) {
                e.printStackTrace()
                //showToast("Error connecting to the server")
            }
        }
    }

    // Step 2: Function to listen for incoming messages from the server
    private suspend fun listenForServerMessages() {
        withContext(Dispatchers.IO) {
            try {
                val buffer = ByteArray(1024)
                var bytesRead: Int

                while (socket?.isConnected == true) {
                    // Reading from the server
                    bytesRead = inputStream?.read(buffer) ?: -1
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
        if ( outputStream == null ) return;

        CoroutineScope(Dispatchers.IO).launch {
            try {
                // Add the packet signature to the buffer.
//                val buffer = ByteBuffer.allocate(bytes.size + 16).order(ByteOrder.LITTLE_ENDIAN)
//                buffer.putInt(PACKET_SIGNATURE[0])
//                buffer.putInt(PACKET_SIGNATURE[1])
//                buffer.putInt(PACKET_SIGNATURE[2])
//                buffer.putInt(PACKET_SIGNATURE[3])
//                buffer.put(bytes)

                outputStream?.write(bytes);
                outputStream?.flush();
            } catch (e: IOException) {
                e.printStackTrace()
                //showToast("Error sending bytes")
            }
        }
    }

    private fun monitorSocket(onConnectionLost: () -> Unit) {
        Thread {
            while (true) {
                if (socket?.isConnected != true) {
                    println("Connection lost!")
                    break
                }
                Thread.sleep(5000) // Check connection every 5 seconds
            }
            onConnectionLost();
        }.start()
    }
}