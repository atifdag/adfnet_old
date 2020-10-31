import { IdCodeName } from '../value-objects/id-code-name';
import { IdCodeNameSelected } from '../value-objects/id-code-name-selected';
import { IdName } from '../value-objects/id-name';

export class UserModel {
    id: string;
    displayOrder: number;
    isApproved: boolean;
    version: number;
    creationTime: Date;
    creator: IdName;
    lastModificationTime: Date;
    lastModifier: IdName;
    username: string;
    password: string;
    email: string;
    identityCode: string;
    firstName: string;
    lastName: string;
    displayName: string;

    roles: IdCodeNameSelected[];
    language: IdName;
    languages: IdName[];
    constructor() {
        this.creator = new IdName();
        this.lastModifier = new IdName();
        this.language = new IdName();
        this.roles = [];
        this.languages = [];
    }
}
