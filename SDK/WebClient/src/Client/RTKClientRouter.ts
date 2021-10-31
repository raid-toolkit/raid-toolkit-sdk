import { ApiDefinition, ApiProvider, Constructor, ISocket, RouterBase } from '@remote-ioc/runtime';
import { RaidToolkitClient } from './RTKClient';

class RTKClientSocket implements ISocket {
  constructor(private readonly scope: string, private readonly socket: Promise<RaidToolkitClient>) {}
  close(): void {
    this.socket.then((socket) => socket.close());
  }
  send<Channel extends string>(channel: Channel, message: any, context?: unknown): this {
    this.socket.then((socket) => socket.send([this.scope, channel, message, context]));
    return this;
  }
  on<Channel extends string>(channel: Channel, handler: (message: any, context?: unknown) => void): this {
    const handlerWrapper = (payload: any) => {
      const [messageScope, messageChannel, messageBody, context] = payload;
      if (channel !== messageChannel || this.scope !== messageScope) {
        return;
      }
      handler(messageBody, context);
    };
    // eslint-disable-next-line no-param-reassign
    (handler as any).handlerWrapper = handlerWrapper;
    this.socket.then((socket) => socket.on('data', handlerWrapper));
    return this;
  }
  off<Channel extends string>(channel: Channel, handler: (message: any, context?: unknown) => void): this {
    this.socket.then((socket) => socket.off('data', (handler as any).handlerWrapper));
    return this;
  }
}

export class RTKClientRouter extends RouterBase {
  private readonly clientReady: Promise<RaidToolkitClient>;
  private readonly discoverSocket: RTKClientSocket;
  constructor(private readonly client: RaidToolkitClient) {
    super();
    this.clientReady = client.waitForConnection();
    // eslint-disable-next-line global-require
    this.discoverSocket = new RTKClientSocket('$router/discover', this.clientReady);
    this.discoverSocket.on('request', this.handleDiscoverRequest);
    this.discoverSocket.on('response', this.handleDiscoverResponse);
    setTimeout(() => {
      this.discoverSocket.send('request', undefined);
    }, 0);
  }

  private handleDiscoverRequest = () => {
    const definitions: string[] = [];
    for (const provider of this.providers) {
      definitions.push(...ApiProvider.implementationsOf(provider).map((def) => ApiDefinition.nameOf(def)));
    }
    this.discoverSocket.send('response', definitions);
  };

  private handleDiscoverResponse = (definitions: string[]) => {
    this.emit('discover', definitions);
  };

  async queryDefinition(definition: Constructor): Promise<boolean> {
    await this.client.connect();
    return super.queryDefinition(definition);
  }

  public getSocketCore(Definition: Constructor<unknown>): ISocket {
    const name = ApiDefinition.nameOf(Definition);
    return new RTKClientSocket(name, this.clientReady);
  }
}
