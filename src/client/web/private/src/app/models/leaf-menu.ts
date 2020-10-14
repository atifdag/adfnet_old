import { ChildMenu } from './child-menu';

export class LeafMenu {
    id: string;
    code: string;
    name: string;
    address: string;
    icon: string;
    description: string;
    parent: ChildMenu;

    constructor() {
        this.parent = new ChildMenu();
    }
}
