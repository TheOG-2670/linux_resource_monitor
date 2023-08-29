package com.example.systemstatsviewclient;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import android.app.AlertDialog;
import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.util.Objects;
import java.util.concurrent.TimeUnit;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import okhttp3.WebSocket;

public class MainActivity extends AppCompatActivity {
    private static final String WEBSOCKET_URL="ws://192.168.68.75:37199/ws";

    WebSocket ws;
    WebSocketListenerCallback callback;
    Button connectButton, disconnectButton;
    TextView tv, connectionStatusTextView;
    OkHttpClient client;
    MainViewModel viewModel;
    StringBuilder stringBuilder;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        connectButton=findViewById(R.id.connectButton);
        disconnectButton=findViewById(R.id.disconnectButton);
        tv=findViewById(R.id.resultText);
        connectionStatusTextView=findViewById(R.id.connectionStatus);

        client=new OkHttpClient.Builder().connectTimeout(20,  TimeUnit.SECONDS).build();
        viewModel = new ViewModelProvider(this).get(MainViewModel.class);
        stringBuilder=new StringBuilder();

        callback=new WebSocketListenerCallback() {
            @Override
            public void websocketSuccess() {
                runOnUiThread(()-> {
                    stringBuilder.append("connected!\n");
                    tv.setText(stringBuilder);
                    connectionStatusTextView.setText(getResources().getText(R.string.connectedStatus));
                });
            }

            @Override
            public void websocketMessageResponse(String text) {
                runOnUiThread(()-> {
                    stringBuilder.append("\nSERVER RESPONSE:\n").append(text);
                    tv.setText(stringBuilder);
                });
            }

            @Override
            public void websocketFailure(WebSocket w, Throwable t, Response r) {
                    runOnUiThread(()->{
                        new AlertDialog.Builder(MainActivity.this)
                                .setTitle("connection failure")
                                .setMessage(t.getMessage())
                                .setNeutralButton("close", (dialog, which) -> dialog.dismiss())
                                .setCancelable(false)
                                .create()
                                .show();
                        ws.close(1000, "websocket session terminated on failure");
                        stringBuilder.append("websocket session terminated on failure\n");
                        tv.setText(stringBuilder);
                        ws=null;
                    });
            }
        };
        WebSocketListener listener=new WebSocketListener(viewModel, callback);

        connectionStatusTextView.setText(getResources().getText(R.string.disconnectedStatus));
        connectButton.setOnClickListener(l->{
            stringBuilder.setLength(0);
            tv.setText(stringBuilder);
            Log.d("okhttp3_websocket", "before newWebSocket called: " + ws);

            ws=client.newWebSocket(new Request.Builder().url(WEBSOCKET_URL).build(), listener);

            Log.d("okhttp3_websocket", "after newWebSocket called: " + ws);
        });

        disconnectButton.setOnClickListener(v -> {
            if(ws==null || Boolean.FALSE.equals(viewModel.getSocketState().getValue())) {
                Toast.makeText(this, "not connected", Toast.LENGTH_SHORT).show();
            }
            else {
                stringBuilder.append("disconnected!\n");
                tv.setText(stringBuilder);
                connectionStatusTextView.setText(getResources().getText(R.string.disconnectedStatus));
                ws.close(1000, "websocket session closed manually");
            }
        });
    }
}
