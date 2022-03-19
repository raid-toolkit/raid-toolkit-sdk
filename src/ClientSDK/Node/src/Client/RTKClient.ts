import { showConnectDialog } from '../UI';

export type EventName = 'data' | 'access-rejected' | 'access-granted' | 'request-access';

export const enum ClientState {
  None,
  Pending,
  Connected,
  Aborted,
}

export class RaidToolkitClient {
  private readonly events: [EventName, (...args: any[]) => void][] = [];
  private channelId?: string;
  private ws?: WebSocket;
  private state: ClientState = ClientState.None;
  private error?: any;
  constructor(private readonly serviceUri = 'wss://raid-toolkit.azurewebsites.net/target') {
    this.serviceUri = serviceUri;
  }

  async connect(): Promise<this> {
    if (!this.ws) {
      await showConnectDialog();
      this.ws = new WebSocket(this.serviceUri);
      this.ws.addEventListener('message', this.onMessage);
      this.state = ClientState.Pending;
    }
    return this.waitForConnection();
  }

  waitForConnection(): Promise<this> {
    switch (this.state) {
      case ClientState.None:
      case ClientState.Pending:
        return new Promise((resolve, reject) =>
          this.once('access-granted', () => resolve(this)).once('access-rejected', reject)
        );
      case ClientState.Aborted:
        return Promise.reject(this.error);
      case ClientState.Connected:
        return Promise.resolve(this);
      default:
        return Promise.reject('internal error');
    }
  }

  close(): any {
    this.ws?.close(0, 'Done');
    delete this.ws;
    delete this.error;
    delete this.channelId;
    this.state = ClientState.None;
  }

  send(message: any) {
    this.ws?.send(
      JSON.stringify({
        type: 'send',
        channelId: this.channelId,
        message,
      })
    );
  }

  on(eventName: EventName, callback: (...args: any[]) => void) {
    this.events.push([eventName, callback]);
    return this;
  }

  once(eventName: EventName, callback: (...args: any[]) => void) {
    this.events.push([
      eventName,
      (...args) => {
        callback(...args);
        this.off(eventName, callback);
      },
    ]);
    return this;
  }

  off(eventName: EventName, callback: (...args: any[]) => void) {
    const idx = this.events.findIndex((e) => e[0] === eventName && e[1] === callback);
    if (idx > -1) {
      this.events.splice(idx, 1);
    }
    return this;
  }

  private emit(eventName: EventName, ...args: any[]) {
    for (const [name, handler] of this.events) {
      if (name === eventName) (handler as any)(...args);
    }
  }

  private onMessage = (event: any) => {
    const msg = JSON.parse(event.data);
    switch (msg.type) {
      case 'ack': {
        this.emit('request-access');
        this.channelId = msg.channelId;
        const href = `rtk://open?channel=${msg.channelId}&origin=${document.location.origin}`;
        document.location.href = href;
        break;
      }
      case 'reject': {
        this.state = ClientState.Aborted;
        this.emit('access-rejected', msg.reason);
        document.body.innerHTML = `Access denied: ${msg.reason}`;
        break;
      }
      case 'accept': {
        this.state = ClientState.Connected;
        this.emit('access-granted');
        break;
      }
      case 'send': {
        this.emit('data', msg.message);
        break;
      }
      default:
        break;
    }
  };
}
