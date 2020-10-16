import { IdCodeName } from '../value-objects/id-code-name';
import { IdCodeNameSelected } from '../value-objects/id-code-name-selected';
import { IdName } from '../value-objects/id-name';

export class UpdateMyInformationModel {
    id: string;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    creationTime: Date;
    creator: IdName;
    lastModificationTime: Date;
    lastModifier: IdName;

    language: IdCodeName;
    languages: IdCodeNameSelected[];
    constructor() {
        this.creator = new IdName();
        this.lastModifier = new IdName();
        this.language = new IdCodeName();
        this.languages = [];
    }
}
