export class Notification {

  id: string = crypto.randomUUID();
  title: string = '';
  description: string = '';
  dateCreate: Date = new Date();
  dateUpdate: Date = new Date();

  constructor(init?: Partial<Notification>) {
    Object.assign(this, init);
  }
}
