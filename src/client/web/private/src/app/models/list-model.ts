import { Paging } from '../value-objects/paging';

export class ListModel<T> {
    paging: Paging;
    items: T[];
    message: string;
    hasError: boolean;

    constructor() {
        this.paging = new Paging();
    }
}
