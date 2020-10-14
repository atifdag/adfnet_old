import { ChildMenu } from './child-menu';

export class RootMenu {
    id: string;
    code: string;
    name: string;
    address: string;
    icon: string;
    description: string;
    childMenus: ChildMenu[];
}
