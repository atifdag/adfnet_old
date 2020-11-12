import { IdCodeName } from '../value-objects/id-code-name';

export class FilterModelWithTwoParent {

    startDate: Date;
    endDate: Date;
    pageNumber: number;
    pageSize: number;
    status: number;
    searched: string;
    parent1: IdCodeName;
    parent2: IdCodeName;
    constructor() {
        this.parent1 = new IdCodeName();
        this.parent2 = new IdCodeName();
    }
}
