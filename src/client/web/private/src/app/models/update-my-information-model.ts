import { IdName } from '../value-objects/id-name';

export class UpdateMyInformationModel {
    id: string;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    biography: string;
    creationTime: Date;
    creator: IdName;
    lastModificationTime: Date;
    lastModifier: IdName;
    constructor() {
        this.creator = new IdName();
        this.lastModifier = new IdName();
    }
}
