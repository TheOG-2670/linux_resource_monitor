package com.example.systemstatsviewclient;

import okhttp3.Response;
import okhttp3.WebSocket;

public interface WebSocketListenerCallback {
    void websocketSuccess(WebSocket w, Response r);
    void websocketFailure(WebSocket w, Throwable t, Response r);
}
