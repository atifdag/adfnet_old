import { IdCodeName } from '../value-objects/id-code-name';

export class FilterModelWithParent {
    parent: IdCodeName;
    startDate: Date;
    endDate: Date;
    pageNumber: number;
    pageSize: number;
    status: number;
    searched: string;

    constructor() {
        this.parent = new IdCodeName();
    }
}
