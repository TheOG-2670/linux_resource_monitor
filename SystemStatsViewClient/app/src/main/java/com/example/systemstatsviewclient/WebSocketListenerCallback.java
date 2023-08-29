package com.example.systemstatsviewclient;

import okhttp3.Response;
import okhttp3.WebSocket;

public interface WebSocketListenerCallback {
    void websocketSuccess();
    void websocketMessageResponse(String text);
    void websocketFailure(WebSocket w, Throwable t, Response r);
}
