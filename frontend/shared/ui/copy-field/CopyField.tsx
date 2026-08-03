import { useState } from "react"
import InputAdornment from "@mui/material/InputAdornment"
import IconButton from "@mui/material/IconButton"
import Tooltip from "@mui/material/Tooltip"
import ContentCopyIcon from "@mui/icons-material/ContentCopy"
import CheckIcon from "@mui/icons-material/Check"
import InputLabel from "@mui/material/InputLabel"
import OutlinedInput from "@mui/material/OutlinedInput"
import FormControl from "@mui/material/FormControl"

const CopyField = ({ value }: { value: string }) => {
	const [copied, setCopied] = useState(false)
	const hasValue = Boolean(value)

	const handleCopy = async () => {
		if (!hasValue) {
			return
		}

		await navigator.clipboard.writeText(value)
		setCopied(true)
		setTimeout(() => {
			setCopied(false)
		}, 2000)
	}

	return (
		<FormControl
			disabled={!hasValue}
			className="flex flex-col gap-2"
			sx={{
				"&:not(.Mui-disabled):hover .MuiInputLabel-root": {
					color: "secondary.contrastText",
				},
			}}
		>
			<InputLabel
				htmlFor="value"
				shrink={hasValue}
				sx={{
					"&:hover": {
						color: "secondary.contrastText",
					},
					"&.MuiFormLabel-colorPrimary.Mui-focused": {
						color: "secondary.contrastText",
					},
				}}
			>
				Код для регистрации
			</InputLabel>
			<OutlinedInput
				label="Код для регистрации"
				notched={hasValue}
				disabled={!hasValue}
				sx={{
					"&:not(.Mui-disabled)": {
						cursor: "pointer",
						"& input": {
							cursor: "pointer",
						},
					},
					"&:not(.Mui-disabled):hover .copy-field-icon": {
						color: "primary.main",
					},
					"& .copy-field-icon": {
						color: "contrastingSecondary.main",
					},
					"& .MuiOutlinedInput-notchedOutline": {
						borderWidth: 1,
					},
					"&:not(.Mui-disabled):hover .MuiOutlinedInput-notchedOutline":
						{
							borderColor: "secondary.contrastText",
						},
					"&.Mui-focused .MuiOutlinedInput-notchedOutline": {
						borderWidth: 1,
						borderColor: "secondary.contrastText",
					},
				}}
				id="value"
				value={value}
				readOnly
				onClick={handleCopy}
				endAdornment={
					<InputAdornment position="end">
						<Tooltip
							open={copied}
							title={copied ? "Скопировано" : "Копировать"}
						>
							<IconButton
								onClick={handleCopy}
								disabled={!hasValue}
								edge="end"
							>
								{copied ? (
									<CheckIcon
										sx={{ color: "success.light" }}
									/>
								) : (
									<ContentCopyIcon className="copy-field-icon" />
								)}
							</IconButton>
						</Tooltip>
					</InputAdornment>
				}
			/>
		</FormControl>
	)
}

export default CopyField
