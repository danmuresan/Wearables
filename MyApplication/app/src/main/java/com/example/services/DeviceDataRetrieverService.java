package com.example.services;

import android.app.Application;
import android.app.IntentService;
import android.content.Intent;
import android.content.Context;
import android.os.Handler;
import android.util.Log;
import android.widget.Toast;

import com.example.danso_000.garmintennisapp.DeviceActivity;
import com.example.danso_000.garmintennisapp.MainActivity;
import com.garmin.android.connectiq.ConnectIQ;
import com.garmin.android.connectiq.IQApp;
import com.garmin.android.connectiq.IQDevice;
import com.garmin.android.connectiq.exception.InvalidStateException;
import com.garmin.android.connectiq.exception.ServiceUnavailableException;

import java.util.List;

/**
 * An {@link IntentService} subclass for handling asynchronous task requests in
 * a service on a separate handler thread.
 * <p>
 * TODO: Customize class - update intent actions, extra parameters and static
 * helper methods.
 */
public class DeviceDataRetrieverService extends IntentService {

    public static final String CONNECT_IQ_DEVICE_KEY = "connectIqDevice";
    public static final String CONNECT_IQ_APP_KEY = "3692F06AABB240CE8F487363F7D6A74B";

    private final ConnectIQ m_connectIQ;
    private IQDevice m_connectIqDevice;
    private IQApp m_iqApp;
    private final ConnectIQ.ConnectIQListener m_connectIqListener;
    private boolean m_connectIqSdkReady;

    public DeviceDataRetrieverService()
    {
        super("DeviceDataRetrieverService");

        m_connectIQ = ConnectIQ.getInstance(this, ConnectIQ.IQConnectType.WIRELESS);
        m_connectIqListener = new BackgroundConnectIqListener();
    }


    @Override
    protected void onHandleIntent(Intent intent) {
        Toast.makeText(this, "Device listener background service started successfully!", Toast.LENGTH_LONG).show();

        // deserialize passed in IQ device and IQ app
        m_connectIqDevice = (IQDevice)intent.getParcelableExtra(DeviceActivity.IQDEVICE);
        try {
            m_connectIQ.getApplicationInfo(CONNECT_IQ_APP_KEY, m_connectIqDevice, new ConnectIQ.IQApplicationInfoListener() {
                @Override
                public void onApplicationInfoReceived(IQApp iqApp) {
                    m_iqApp = iqApp;
                    m_connectIQ.initialize(getApplicationContext(), true, m_connectIqListener);
                }

                @Override
                public void onApplicationNotInstalled(String s) {
                    Toast.makeText(getApplicationContext(), "ConnectIq app not installed on your device!", Toast.LENGTH_LONG).show();
                }
            });
        } catch (InvalidStateException e) {
            e.printStackTrace();
        } catch (ServiceUnavailableException e) {
            e.printStackTrace();
        }
    }

    private class BackgroundConnectIqListener implements ConnectIQ.ConnectIQListener {

        @Override
        public void onSdkReady() {
            m_connectIqSdkReady = true;

            try {

                m_connectIQ.registerForAppEvents(m_connectIqDevice, m_iqApp, new ConnectIQ.IQApplicationEventListener() {
                    @Override
                    public void onMessageReceived(IQDevice iqDevice, IQApp iqApp, List<Object> list, ConnectIQ.IQMessageStatus iqMessageStatus) {
                        switch (iqMessageStatus)
                        {
                            case SUCCESS:
                                // TODO: do something here

                                String fullMsg = "";
                                for (Object o : list)
                                {
                                    fullMsg += (String)o;
                                }
                                Toast.makeText(getApplicationContext(), fullMsg, Toast.LENGTH_SHORT).show();
                                Log.d("MB MESSAGE", "Successfully received msg");
                                break;
                            case FAILURE_MESSAGE_TOO_LARGE:
                                // TODO:
                                Log.d("MB MESSAGE", "Message too large");
                                break;
                            case FAILURE_INVALID_FORMAT:
                                // TODO:
                                Log.d("MB MESSAGE", "Message in invalid format");
                                break;
                        }

                    }
                });
            } catch (InvalidStateException e) {
                e.printStackTrace();
            }
        }

        @Override
        public void onInitializeError(ConnectIQ.IQSdkErrorStatus iqSdkErrorStatus) {

        }

        @Override
        public void onSdkShutDown() {
            m_connectIqSdkReady = false;
            try {
                m_connectIQ.unregisterForApplicationEvents(m_connectIqDevice, m_iqApp);
            } catch (InvalidStateException e) {
                e.printStackTrace();
            }
        }

    }
}
