export enum StatusOption {
    Approved = 1,
    NotApproved = 0,
    All = -1
}

export namespace StatusOption {

    export function values() {
        return Object.keys(StatusOption).filter(
            (type) => isNaN(<any>type) && type !== 'values'
        );
    }
}
