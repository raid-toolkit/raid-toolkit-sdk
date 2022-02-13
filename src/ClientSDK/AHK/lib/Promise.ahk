class Promise
{
    IsPromise := true
    Result :=
    Error :=
    OnSuccess := Array()
    OnFailure := Array()
    OnFinally := Array()
    ChildPromises := Array()

    __New(Result:="",Error:="")
    {
        this.Result := Result
        this.Error := Error
    }

    _InvokeCallback(Callback, Value)
    {
        Try
        {
            CallbackResult := %Callback%(Value)
            If (CallbackResult.IsPromise)
            {
                For idx,Child in this.ChildPromises
                    CallbackResult.Then(ObjBindMethod(Child, "_SetResult"), ObjBindMethod(Child, "_SetError"))
            }
            Else
            {
                For idx,Child in this.ChildPromises
                    Child._SetResult(CallbackResult)
            }
        }
        Catch Error
        {
            For idx,Child in this.ChildPromises
                Child._SetError(Error)
        }
        Return ResultPromise
    }

    Then(OnSuccess, OnFailure := "")
    {
        PromiseResult := new Promise()
        this.ChildPromises.Push(PromiseResult)

        If (this.Result)
        {
            this._InvokeCallback(OnSuccess, this.Result)
            Return
        }
        
        this.OnSuccess.Push(OnSuccess)

        If (OnFailure)
            this.Catch(OnFailure)

        return PromiseResult
    }

    Catch(OnFailure)
    {
        PromiseResult := new Promise()
        this.ChildPromises.Push(PromiseResult)

        this._Catch(OnFailure)

        this.OnFailure.Push(OnFailure)
        
        return PromiseResult
    }
    
    _Catch(OnFailure)
    {
        If (!OnFailure)
            Return

        If (this.Error)
        {
            this._InvokeCallback(OnFailure, this.Error)
            Return
        }

        this.OnFailure.Push(OnFailure)
    }
    
    Finally(OnFinally)
    {
        If (this.Error or this.Result)
        {
            this._InvokeCallback(OnFinally)
            Return
        }

        this.OnFinally.Push(OnFinally)
    }

    _SetResult(Result)
    {
        this.Result := Result
        for k,v in this.OnSuccess
            this._InvokeCallback(v, Result)

        for k,v in this.OnFinally
            this._InvokeCallback(v, Result)
    }

    _SetError(Error)
    {
        this.Error := Error
        for k,v in this.OnFailure
            this._InvokeCallback(v, Error)

        for k,v in this.OnFinally
            this._InvokeCallback(v, Result)
    }
}
