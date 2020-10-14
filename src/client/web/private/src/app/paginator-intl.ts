import { MatPaginatorIntl } from '@angular/material/paginator';

const dutchRangeLabel = (page: number, pageSize: number, length: number) => {
  if (length === 0 || pageSize === 0) { return `${length} kayıttan 0`; }

  length = Math.max(length, 0);

  const startIndex = page * pageSize;

  // If the start index exceeds the list length, do not try and fix the end index to the end.
  const endIndex = startIndex < length ?
    Math.min(startIndex + pageSize, length) :
    startIndex + pageSize;
  return `${length} kayıttan ${startIndex + 1} ile ${endIndex} arası`;
}


export function getPaginatorIntl(): MatPaginatorIntl {
  const paginatorIntl = new MatPaginatorIntl();

  paginatorIntl.itemsPerPageLabel = 'Sayfada Gösterilecek Kayıt:';
  paginatorIntl.nextPageLabel = 'Sonraki Sayfa';
  paginatorIntl.previousPageLabel = 'Önceki Sayfa';
  paginatorIntl.getRangeLabel = dutchRangeLabel;

  return paginatorIntl;
}
