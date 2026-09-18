import { storeCreate } from "@/shared/lib/storeCreate"
import { TDiscipline } from "./TDiscipline"

const useDisciplineStore = storeCreate<TDiscipline[], "disciplines">(
	"disciplines",
	[],
)

export default useDisciplineStore
