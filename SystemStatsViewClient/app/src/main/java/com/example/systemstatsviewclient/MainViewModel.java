package com.example.systemstatsviewclient;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

public class MainViewModel extends ViewModel {
    private final MutableLiveData<Boolean> socketState = new MutableLiveData<>();
    public LiveData<Boolean> getSocketState() {
        return socketState;
    }

    public void setSocketState(Boolean _socketConnection) {
        socketState.postValue(_socketConnection);
    }
}
