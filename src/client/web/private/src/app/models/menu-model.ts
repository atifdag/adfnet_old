import { IdCodeName } from '../value-objects/id-code-name';
import { IdName } from '../value-objects/id-name';

export class MenuModel {
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
    address: string;
    icon: string;
    parentMenu: IdCodeName;
    constructor() {
        this.creator = new IdName();
        this.lastModifier = new IdName();
        this.parentMenu = new IdCodeName();
    }
}
