import { IdName } from '../value-objects/id-name';

export class ParameterGroupModel {
    id: string;
    displayOrder: number;
    isApproved: boolean;
    version: number;
    creationTime: Date;
    creator: IdName;
    lastModificationTime: Date;
    lastModifier: IdName;
    code: string;
    name: string;
    description: string;

    constructor() {
        this.creator = new IdName();
        this.lastModifier = new IdName();
    }
}
