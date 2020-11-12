import { IdCodeName } from '../value-objects/id-code-name';

export class LanguageModel {
    id: string;
    displayOrder: number;
    isApproved: boolean;
    version: number;
    creationTime: Date;
    lastModificationTime: Date;
    code: string;
    name: string;
    description: string;

}
