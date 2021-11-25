class PromiseStore
{
    PromiseIndex := 1
    PromisePrefix := "AHK_" . Mod(A_TickCount, 50000) . "_"
    Promises := {}

    Create()
    {
        id := this.PromiseIndex
        this.PromiseIndex += 1
        PromiseId := % this.PromisePrefix . this.PromiseIndex
        this.Promises[PromiseId] := new Promise()
        return PromiseId
    }

    Complete(PromiseId, Value)
    {
        this.Promises[PromiseId]._SetResult(Value)
    }

    Fail(PromiseId, Error)
    {
        this.Promises[PromiseId]._SetError(Error)
    }

    Get(PromiseId)
    {
        return this.Promises[PromiseId]
    }
}