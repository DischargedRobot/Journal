import TextField from "@mui/material/TextField"
import { HTMLInputTypeAttribute } from "react"

interface Props {
	label: string
	size: "small" | "medium"
	type: HTMLInputTypeAttribute
	errorMessage?: string
	helperText?: string
	required: boolean
}

const FormTextField = (props: Props) => {
	const { label, size, type, errorMessage, helperText, required } = props

	return (
		<TextField
			variant="outlined"
			label={label}
			size={size}
			type={type}
			required={required}
			error={!!errorMessage}
			helperText={helperText ?? " "}
		/>
	)
}

export default FormTextField
