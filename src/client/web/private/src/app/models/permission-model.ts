import { IdCodeName } from '../value-objects/id-code-name';
import { IdCodeNameSelected } from '../value-objects/id-code-name-selected';
import { IdName } from '../value-objects/id-name';

export class PermissionModel {
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
    controllerName: string;
    actionName: string;
    menus: IdCodeNameSelected[];
    constructor() {
        this.creator = new IdName();
        this.lastModifier = new IdName();
        this.menus = [];
    }

}
