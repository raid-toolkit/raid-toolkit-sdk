#Include, <WebSocket>
#Include, <jxon>
#Include, <Promise>
#Include, <PromiseStore>

class RemoteApiClient extends WebSocket
{
    _Promises := New PromiseStore
    _Ready := false

    _Call(ApiName, Method, Parameters:="")
    {
        while !this._Ready
            Sleep, 50

        PromiseId := this._Promises.Create()

        args := "[]"
        if (Parameters and Parameters.MaxIndex() > 0)
        {
            args := Jxon_Dump(Parameters)
        }

        Payload := "[""" . ApiName . """,""call"",{""promiseId"":""" . PromiseId . """,""methodName"":""" . Method . """,""args"":" . args . "}]"
        this.Send(Payload)
        return this._Promises.Get(PromiseId)
    }

	OnOpen(Event)
	{
        this._Ready := true
	}
	
	OnMessage(Event)
	{
        Callback := ObjBindMethod(this, "HandleMessage", Event)
        SetTimer % Callback, -1
	}

	HandleMessage(Event)
	{
        Message := Jxon_Load(Event.data)
        if (Message[2] == "set-promise")
        {
            Value := Message[3]
            if (!Value.error)
            {
                this._Promises.Complete(Value.promiseId, Value.value)
            }
            else
            {
                this._Promises.Fail(Value.promiseId, Value.error)
            }
        }
	}
	
	OnClose(Event)
	{
		MsgBox, Websocket Closed
		this.Disconnect()
	}
	
	OnError(Event)
	{
	}
}