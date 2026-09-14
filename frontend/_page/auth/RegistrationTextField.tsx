import TextField from "@mui/material/TextField"
import { HTMLInputTypeAttribute, memo, ReactNode } from "react"
import useRegistrationFormStore, {
	TFormValues,
} from "./model/useRegistrationFormStore"
import { PasswordStregth } from "@/shared/ui/PasswordStregth"

interface Props {
	label: string
	size: "small" | "medium"
	type?: HTMLInputTypeAttribute
	errorMessage?: string
	helperText?: ReactNode
	required?: boolean
	fieldName: keyof TFormValues
}

const RegistrationTextField = (props: Props) => {
	const { label, size, type, errorMessage, helperText, required, fieldName } =
		props

	const field = useRegistrationFormStore(
		(state) => state.formValues[fieldName],
	)
	const updateField = useRegistrationFormStore((state) => state.updateField)

	let visibleHelperText: ReactNode =
		(helperText ?? field?.error.length > 0) ? field.error : " "
	if (fieldName == "password") {
		visibleHelperText = <PasswordStregth password={field.value ?? ""} />
	}
	// console.log(visibleHelperText)

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

export default memo(RegistrationTextField)
