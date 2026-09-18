import { storeCreate } from "@/shared/lib/storeCreate"
import { TUser } from "./TUser"

export const useUserStore = storeCreate<TUser, "user">("user", {
	uuid: "",
	email: "",
	firstName: "Гость",
	lastName: "",
	patronymic: "",
	role: {
		uuid: "",
		name: "Гость",
		rights: [],
		roleTypes: [],
		isBase: true,
	},
	groups: [],
	departments: [],
	version: 0,
})
