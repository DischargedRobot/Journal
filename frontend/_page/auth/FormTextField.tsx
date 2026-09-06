import TextField from "@mui/material/TextField"
import { HTMLInputTypeAttribute, memo } from "react"
import useRegistrationFormStore, {
	TFormValues,
} from "./model/useRegistrationFormStore"

interface Props {
	label: string
	size: "small" | "medium"
	type?: HTMLInputTypeAttribute
	errorMessage?: string
	helperText?: string
	required?: boolean
	fieldName: keyof TFormValues
}

const FormTextField = (props: Props) => {
	const { label, size, type, errorMessage, helperText, required, fieldName } =
		props

	const field = useRegistrationFormStore(
		(state) => state.formValues[fieldName],
	)
	const updateField = useRegistrationFormStore((state) => state.updateField)

	const visibleHelperText =
		(helperText ?? field?.error.length > 0) ? field.error : " "
	console.log(visibleHelperText)

	return (
		<TextField
			variant="outlined"
			label={label}
			size={size}
			type={type}
			required={required ?? field?.required}
			error={errorMessage ? !!errorMessage : !!field?.error}
			helperText={visibleHelperText}
			value={field?.value ?? ""}
			onChange={(event) => updateField(fieldName, event.target.value)}
		/>
	)
}

export default memo(FormTextField)
