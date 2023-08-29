package com.example.systemstatsviewclient;

import android.util.Log;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import okhttp3.Response;
import okhttp3.WebSocket;

public class WebSocketListener extends okhttp3.WebSocketListener {
    private final String TAG="okhttp3_websocket";
    private final MainViewModel viewModel;
    private final WebSocketListenerCallback callback;

    public WebSocketListener(MainViewModel _viewModel, WebSocketListenerCallback callback) {
        viewModel=_viewModel;
        this.callback=callback;
    }

    @Override
    public void onOpen(@NonNull WebSocket webSocket, @NonNull Response response) {
        super.onOpen(webSocket, response);
        viewModel.setSocketState(true);
        Log.d(TAG, "socket opened!");
        callback.websocketSuccess();
    }

    @Override
    public void onMessage(@NonNull WebSocket webSocket, @NonNull String text) {
        super.onMessage(webSocket, text);
        Log.d(TAG, "message: "+text);
        callback.websocketMessageResponse(text);
    }

    @Override
    public void onClosing(@NonNull WebSocket webSocket, int code, @NonNull String reason) {
        super.onClosing(webSocket, code, reason);
        Log.d(TAG, "closing");
    }

    @Override
    public void onClosed(@NonNull WebSocket webSocket, int code, @NonNull String reason) {
        super.onClosed(webSocket, code, reason);
        viewModel.setSocketState(false);
        Log.d(TAG, "closed: " + reason);
    }

    @Override
    public void onFailure(@NonNull WebSocket webSocket, @NonNull Throwable t, @Nullable Response response) {
        super.onFailure(webSocket, t, response);
        Log.d(TAG, "socket failure!\n"+t.getMessage() + " " + response);
        callback.websocketFailure(webSocket, t, response);
    }
}
