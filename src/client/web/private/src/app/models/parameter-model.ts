import { IdCodeName } from '../value-objects/id-code-name';
import { IdName } from '../value-objects/id-name';

export class ParameterModel {
    id: string;
    displayOrder: number;
    isApproved: boolean;
    version: number;
    creationTime: Date;
    creator: IdName;
    lastModificationTime: Date;
    lastModifier: IdName;
    key: string;
    value: string;
    erasable: boolean;
    description: string;
    parameterGroup: IdCodeName;
    constructor() {
        this.creator = new IdName();
        this.lastModifier = new IdName();
        this.parameterGroup = new IdCodeName();
    }
}
