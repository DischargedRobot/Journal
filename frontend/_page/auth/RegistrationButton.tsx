import Button from "@mui/material/Button"
import useRegistrationFormStore from "./model/useRegistrationFormStore"
import { memo } from "react"
import { REQUIRED_FIELDS } from "./fields"

const RegistrationButton = () => {
	const isValid = useRegistrationFormStore((state) => {
		return REQUIRED_FIELDS.every((fieldName) => {
			const field = state.formValues[fieldName]

			return field?.value?.trim() !== "" && field?.error === ""
		})
	})

	return (
		<Button
			className="w-full"
			variant="contained"
			color="primary"
			type="submit"
			disabled={!isValid}
		>
			Зарегистрироваться
		</Button>
	)
}

export default memo(RegistrationButton)
