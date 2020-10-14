import { RootMenu } from './root-menu';
import { LeafMenu } from './leaf-menu';

export class ChildMenu {
    id: string;
    code: string;
    name: string;
    address: string;
    icon: string;
    description: string;
    parent: RootMenu;
    leafMenus: LeafMenu[];
    constructor() {
        this.parent = new RootMenu();
    }
}
