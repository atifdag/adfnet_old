import { UserModel } from './user-model';
import { RootMenu } from './root-menu';

export class MyProfileModel {
    userModel: UserModel;
    message: string;
    lastLoginTime: Date;
    rootMenus: RootMenu[];
    constructor() {
        this.userModel = new UserModel();
    }
}
