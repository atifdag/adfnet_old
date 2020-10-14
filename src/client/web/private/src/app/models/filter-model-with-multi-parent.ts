import { IdCodeName } from '../value-objects/id-code-name';
import { IdCodeNameSelected } from '../value-objects/id-code-name-selected';

export class FilterModelWithMultiParent {
    parent: IdCodeName;
    startDate: Date;
    endDate: Date;
    pageNumber: number;
    pageSize: number;
    status: number;
    searched: string;
    parents: IdCodeNameSelected[];
    constructor() {
        this.parents = [];
    }
}
