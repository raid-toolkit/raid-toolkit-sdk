import { methodStub } from '@remote-ioc/runtime';

export class TypedEventEmitter<TEventTuples extends { [key: string]: any[] }> {
  on<T extends keyof TEventTuples>(type: T, callback: (...args: TEventTuples[T]) => void): this {
    methodStub(this, type, callback);
  }
  off<T extends keyof TEventTuples>(type: T, callback: (...args: TEventTuples[T]) => void): this {
    methodStub(this, type, callback);
  }
}
