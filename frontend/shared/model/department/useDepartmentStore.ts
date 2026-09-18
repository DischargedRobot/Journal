import { storeCreate } from "@/shared/lib/storeCreate"
import { TDepartment } from "./TDepartment"

export const useDepartmentStore = storeCreate<TDepartment[], "departments">(
	"departments",
	[],
)
